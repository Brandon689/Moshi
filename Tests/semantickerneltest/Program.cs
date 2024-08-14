using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Kernel kernel = Kernel.CreateBuilder()
           .AddOpenAIChatCompletion(
               modelId: "gpt-4o-mini",
               apiKey: "key-here")
           .Build();

// Example 1. Invoke the kernel with a prompt and display the result
Console.WriteLine(await kernel.InvokePromptAsync("What color is the sky?"));
Console.WriteLine();
/*
The sky appears blue during the day due to the scattering of
sunlight by the Earth's atmosphere. This phenomenon, known as
Rayleigh scattering, causes shorter blue wavelengths of light to
be scattered more than other colors. However, the sky can also
appear in various colors at different times, such as orange and
red during sunrise and sunset, or gray when overcast.
*/


// Example 2. Invoke the kernel with a templated prompt and display the result
KernelArguments arguments = new() { { "topic", "sea" } };
Console.WriteLine(await kernel.InvokePromptAsync("What color is the {{$topic}}?", arguments));
Console.WriteLine();
/*
The color of the sea can vary greatly depending on several factors,
including the time of day, the depth of the water, the presence of
algae or plankton, and the composition of the seabed. Generally,
the sea appears blue due to the absorption and scattering of
sunlight, with deeper waters appearing darker. In shallow areas,
the sea may appear turquoise or greenish due to the reflection of
light from the seabed. Additionally, in coastal regions, the water
can appear brownish or muddy due to sediments. So, while blue is a
common color associated with the sea, it can take on many shades
and hues.
*/


// Example 3. Invoke the kernel with a templated prompt and stream the results to the display
KernelArguments arguments2 = new() { { "topic", "sun" } };
await foreach (var update in kernel.InvokePromptStreamingAsync("What color is the {{$topic}}? Provide a detailed explanation.", arguments2))
{
    Console.Write(update);
}
Console.WriteLine(string.Empty);
/*
The color of the sun can actually be described in multiple ways,
depending on various factors such as context, perspective, and
atmospheric conditions.

    The sun emits light across a broad spectrum of wavelengths, the
    most intense being in the visible range. If you look at the sun
    from space, it appears white because it emits all colors of light
    relatively evenly. The combination of all these colors results in
    white light, similar to how a prism can split white light into a
    rainbow of colors.
    When observed from Earth's surface, the color of the sun can
    change due to the atmosphere. During midday, when the sun is
    overhead, it may appear closer to white. However, during sunrise
    or sunset, the sun often appears yellow, orange, or red. This is
    because the sunlight must pass through more of the Earth's
    atmosphere at these times. The shorter blue and violet wavelengths
    are scattered out of the direct path of light (Rayleigh
    scattering), leaving the longer wavelengths (reds and oranges)
    more dominant.
    Colors can also carry different meanings culturally or
    psychologically. People often associate the sun with colors like
    yellow and gold, which can evoke feelings of warmth and
    positivity. In art and literature, the sun is frequently depicted
    in these warmer hues.
    The way humans perceive color can also influence the sun's
    appearance. Our eyes are more sensitive to certain wavelengths,
    and our brain interprets these signals. The sun's brightness may
    wash out some of the colors, leading to the perception of a more
    yellowish or whitish color.

In summary, while the sun technically emits white light, its
apparent color can vary significantly based on environmental
conditions, atmospheric effects, and human perception, ranging
from white to yellow to orange to red.
*/


// Example 4. Invoke the kernel with a templated prompt and execution settings
KernelArguments arguments3 = new(new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.5 }) { { "topic", "dogs" } };
Console.WriteLine(await kernel.InvokePromptAsync("Tell me a story about {{$topic}}", arguments3));
/*
Once upon a time in a small, sunny village nestled between rolling
hills, there lived a pack of dogs known as the "Happytails." Each
member of the pack had a unique personality and a special talent
that made them beloved by the villagers. At the forefront was Max, a golden retriever with a heart of gold.
Max was known for his playful spirit and his ability to lift
anyone's mood. He would often lead the pack in games of fetch,
bringing joy to children and adults alike. Then there was Bella, a clever border collie with a knack for
problem-solving. Bella loved to help the villagers with their
chores, whether it was herding sheep or finding lost items. Her
intelligence and quick thinking made her the unofficial "helper"
of the village. Next came Rocky, a brave bulldog with a protective nature. Rocky
took it upon himself to watch over the village, ensuring that
everyone was safe. He was especially fond of the little ones and
would often accompany them on their adventures, keeping a watchful
eye. The last member of the pack was Daisy, a gentle beagle with a nose
for adventure. Daisy loved to explore the woods surrounding the
village, always discovering new trails and hidden treasures. Her
curiosity often led the pack on exciting escapades. One sunny day, the villagers decided to host a festival to
celebrate the bond between them and their furry friends. They
organized games, races, and even a talent show where the dogs
could showcase their unique skills. The entire village buzzed with
excitement as everyone prepared for the big day. As the festival began, Max led the pack in a joyful parade,
wagging his tail and barking happily. Bella wowed the crowd with
her incredible agility, weaving through obstacles with grace.
Rocky proudly stood guard at the entrance, ensuring that everyone
felt safe and welcome. And Daisy, with her adventurous spirit, led
a scavenger hunt that took the villagers on a thrilling journey
through the woods. As the sun began to set, the villagers gathered around a large
bonfire, sharing stories and laughter. The dogs snuggled close to
their humans, enjoying the warmth and love that surrounded them.
It was a perfect ending to a perfect day. From that day on, the Happytails pack became an inseparable part
of the village, reminding everyone of the joy, loyalty, and love
that dogs bring into our lives. And as the seasons changed, so did
their adventures, but one thing remained constant: the unbreakable
*/